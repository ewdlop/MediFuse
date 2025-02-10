using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MedicalNER
{
    // Data structure for input medical text
    public class MedicalText
    {
        [LoadColumn(0)]
        public string Text { get; set; }
        
        [LoadColumn(1)]
        public string[] Tokens { get; set; }
        
        [LoadColumn(2)]
        public string[] Labels { get; set; }
    }

    // Data structure for predictions
    public class MedicalEntityPrediction
    {
        [ColumnName("PredictedLabel")]
        public string[] PredictedLabels { get; set; }
    }

    public class MedicalNERSystem
    {
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;
        private readonly string _modelPath = "{medical_ner_model}.zip";

        public MedicalNERSystem()
        {
            _mlContext = new MLContext(seed: 42);
        }

        public void Train(string trainingDataPath)
        {
            // Load and preprocess training data
            var trainingData = LoadAndPreprocessData(trainingDataPath);

            // Create data processing pipeline
            var pipeline = CreateTrainingPipeline();

            // Train the model
            _trainedModel = pipeline.Fit(trainingData);

            // Save the model
            SaveModel(trainingData.Schema);
        }

        private IDataView LoadAndPreprocessData(string dataPath)
        {
            // Load data from file
            var dataView = _mlContext.Data.LoadFromTextFile<MedicalText>(
                dataPath,
                separatorChar: '\t',
                hasHeader: true
            );

            // Preprocess data
            var preprocessPipeline = _mlContext.Transforms.Conversion.MapValueToKey("Labels")
                .Append(_mlContext.Transforms.Text.TokenizeIntoWords("TokenizedText", "Text"))
                .Append(_mlContext.Transforms.Text.NormalizeText("NormalizedText", "TokenizedText"))
                .Append(_mlContext.Transforms.Text.FeaturizeText("Features", "NormalizedText"));

            return preprocessPipeline.Fit(dataView).Transform(dataView);
        }

        private EstimatorChain<Microsoft.ML.Transforms.KeyToValueMappingEstimator> CreateTrainingPipeline()
        {
            // Define the training pipeline
            var pipeline = _mlContext.MulticlassClassification.Trainers
                .OneVersusAll(_mlContext.BinaryClassification.Trainers.AveragedPerceptron())
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            return pipeline;
        }

        private void SaveModel(DataViewSchema trainingScheme)
        {
            _mlContext.Model.Save(_trainedModel, trainingScheme, _modelPath);
        }

        public void LoadModel()
        {
            if (File.Exists(_modelPath))
            {
                _trainedModel = _mlContext.Model.Load(_modelPath, out _);
            }
            else
            {
                throw new FileNotFoundException("Model file not found. Please train the model first.");
            }
        }

        public IEnumerable<(string Entity, string Type)> PredictEntities(string text)
        {
            // Create prediction engine
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MedicalText, MedicalEntityPrediction>(_trainedModel);

            // Tokenize input text
            var tokens = text.Split(' ');
            var input = new MedicalText
            {
                Text = text,
                Tokens = tokens
            };

            // Make prediction
            var prediction = predictionEngine.Predict(input);

            // Process predictions and return entities
            return ProcessPredictions(tokens, prediction.PredictedLabels);
        }

        private IEnumerable<(string Entity, string Type)> ProcessPredictions(string[] tokens, string[] labels)
        {
            var entities = new List<(string Entity, string Type)>();
            var currentEntity = new List<string>();
            string currentType = null;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (labels[i].StartsWith("B-")) // Beginning of entity
                {
                    if (currentEntity.Count > 0)
                    {
                        entities.Add((string.Join(" ", currentEntity), currentType));
                        currentEntity.Clear();
                    }
                    currentType = labels[i].Substring(2);
                    currentEntity.Add(tokens[i]);
                }
                else if (labels[i].StartsWith("I-") && currentEntity.Count > 0) // Inside entity
                {
                    currentEntity.Add(tokens[i]);
                }
                else // Outside entity
                {
                    if (currentEntity.Count > 0)
                    {
                        entities.Add((string.Join(" ", currentEntity), currentType));
                        currentEntity.Clear();
                        currentType = null;
                    }
                }
            }

            // Add last entity if exists
            if (currentEntity.Count > 0)
            {
                entities.Add((string.Join(" ", currentEntity), currentType));
            }

            return entities;
        }
    }

    // Example usage
    class Program
    {
        static void Main(string[] args)
        {
            var nerSystem = new MedicalNERSystem();

            // Train the model
            nerSystem.Train("{path_to_training_data}.txt");

            // Example prediction
            var text = "Patient was prescribed Metformin 500mg for Type 2 Diabetes Mellitus (E11.9)";
            var entities = nerSystem.PredictEntities(text);

            foreach (var (entity, type) in entities)
            {
                Console.WriteLine($"Entity: {entity}, Type: {type}");
            }
        }
    }
}

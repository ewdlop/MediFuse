namespace MediFuseClassLibrary

// Define the discriminated union type in a module
module MyUnion =
    type y<'a, 'b> =
        | A of 'a
        | B of 'b

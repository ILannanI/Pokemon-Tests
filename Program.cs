using System;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter a Pokémon name or ID (e.g., pikachu or 25): ");
        string pokemonInput = Console.ReadLine();

        var pokemon = await GetPokemonAsync(pokemonInput);

        if (pokemon != null)
        {
            Console.WriteLine($"Name: {pokemon.Name}");
            Console.WriteLine($"Height: {pokemon.Height} decimeters");
            Console.WriteLine($"Weight: {pokemon.Weight} hectograms");
        }
        else
        {
            Console.WriteLine("Pokémon not found.");
        }
    }

    static async Task<Pokemon> GetPokemonAsync(string nameOrId)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = $"https://pokeapi.co/api/v2/pokemon/{nameOrId.ToLower()}";
                string jsonResponse = await client.GetStringAsync(url);
                var pokemon = JsonConvert.DeserializeObject<Pokemon>(jsonResponse);
                return pokemon;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}

public class Pokemon
{
    public string Name { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
}


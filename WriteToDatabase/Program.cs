using Npgsql;
using NpgsqlTypes;

public class WriteToDatabase
{
    public static void Main() 
    {
        string connectionStrig; // @"Host=localhost;Port=5432;Username=postgres;Password=?;Database=postgres;Trust Server Certificate=True"
        string query;           // What table and column you want to insert to
        string directoryFile;   // directory to the file you want to read from
    }

    public static void WriteFromFileToDatabase(string connectionString, string query, string directoryToFile) 
    {
        string swedish_words = @directoryToFile;
        string words = File.ReadAllText(swedish_words);
        string[] word_list = words.Split("\n");
        query = "INSERT INTO lexicon(word) VALUES (@word)";
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        using (NpgsqlCommand commnd = new NpgsqlCommand(query, connection))
        {
            connection.Open();

            commnd.Parameters.Add("@word", NpgsqlDbType.Varchar).Value = word_list;

            commnd.ExecuteNonQuery();
        }  
    }
}
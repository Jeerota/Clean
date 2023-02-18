namespace Clean.Generator.Helpers
{
    public class FileEditor
    {
        public static string ReadTemplateText(string templateDirectory, string templateLocation)
        {
            StreamReader reader = new($"{templateDirectory}\\{templateLocation}");
            string text = reader.ReadToEnd();
            reader.Close();
            return text;
        }

        public static string ReadEntityText(string saveLocation, string tableName)
        {
            StreamReader reader = new($"{saveLocation}\\Entities\\{tableName}.cs");
            string text = reader.ReadToEnd();
            reader.Close();
            return text;
        }

        public static void WriteFile(string fileLocation, string fileName, string fileContent)
        {
            if (!new FileInfo(fileLocation).Exists)
                Directory.CreateDirectory(fileLocation);

            StreamWriter writer = new($"{fileLocation}\\{fileName}");
            writer.Write(fileContent);
            writer.Flush();
            writer.Close();
        }
    }
}

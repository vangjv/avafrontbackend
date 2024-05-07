using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AvaFront.API.Helpers
{
    public static class KeyVaultHelper
    {
        public static void LoadSecretsToEnvironment(string vaultUri)
        {
            // Use DefaultAzureCredential which supports managed identity
            var credential = new DefaultAzureCredential();

            var client = new SecretClient(new Uri(vaultUri), credential);

            // List of your secrets
            string[] secretNames = { "OpenAIApiKey", "RedisConnectionString", "gpt4--ApiKey", "gpt35turbo--ApiKey", "AvaFrontDatabase--PrimaryKey" };

            foreach (var secretName in secretNames)
            {
                KeyVaultSecret secret = client.GetSecret(secretName);
                Environment.SetEnvironmentVariable(secretName, secret.Value);
            }
        }
    }
}

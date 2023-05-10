using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace minimalapi
{
    public class ConfigReader : IDisposable
    {
        private bool disposedValue;
        private readonly string _jsonFile;

        public ConfigReader(string jsonFile)
        {
            _jsonFile = jsonFile;
        }

        public async Task<(string, string)> ReadAsync()
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, _jsonFile);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(fullPath);
            }

            var configString = await File.ReadAllTextAsync(fullPath);

            var jsonNode = JsonNode.Parse(configString);
            var url = jsonNode!["url"]!.ToString();
            var key = jsonNode!["key"]!.ToString();

            return (url, key);
        }

        #region IDisposble implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ConfigReader()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

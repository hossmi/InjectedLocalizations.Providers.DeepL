using System;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class DeepLConfigurationException : ArgumentException
    {
        public DeepLConfigurationException(string paramName)
            : base($"Missing DeepL configuration parameter '{paramName}'", paramName) { }

        protected DeepLConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

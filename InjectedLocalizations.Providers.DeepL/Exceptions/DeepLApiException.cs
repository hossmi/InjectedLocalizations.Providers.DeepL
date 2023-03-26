using System;
using System.Runtime.Serialization;
using DeepL;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class DeepLApiException : LocalizationException
    {
        public DeepLApiException() : base($"Cannot get {typeof(Translator)} instance.")
        {
        }

        protected DeepLApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

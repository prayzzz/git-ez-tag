using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Git.Ez.Tag.Services
{
    public class AnnotationService
    {
        private readonly ILogger<AnnotationService> _logger;

        public AnnotationService(ILogger<AnnotationService> logger)
        {
            _logger = logger;
        }

        public string GetAnnotation(string nextTag)
        {
            return Prompt.GetString($"> What's your annotation for Tag '{nextTag}'?", nextTag);
        }
    }
}
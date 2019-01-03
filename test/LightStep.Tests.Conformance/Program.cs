using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTracing.Propagation;

namespace LightStep.Tests.Conformance
{
    class ConformanceClient
    {
        static void Main(string[] args)
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var json = JObject.Parse(line);
                var textMap = json.Value<JObject>("text_map").Properties();
                var textMapExtractCarrier = textMap.ToDictionary(k => k.Name, v => v.Value.ToString());
               
                var tracer = new Tracer(new Options("invalid").WithAutomaticReporting(false));

                var ctx = tracer.Extract(BuiltinFormats.TextMap, new TextMapExtractAdapter(textMapExtractCarrier));
                
                var injectCarrier = new Dictionary<String, String>();
                tracer.Inject(ctx, BuiltinFormats.TextMap, new TextMapInjectAdapter(injectCarrier));
                
                var textMapCarrierString = JsonConvert.SerializeObject(injectCarrier);
                Console.WriteLine($"{{\"text_map\": {textMapCarrierString}, \"binary\": \"\"}}");
            }
        }
    }
}
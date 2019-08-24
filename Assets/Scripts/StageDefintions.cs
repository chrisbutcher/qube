using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class StageDefinitions {
  [JsonProperty("stages")]
  public List<Stage> Stages { get; set; }
}

public partial class Stage {
  [JsonProperty("name")]
  public long Name { get; set; }

  [JsonProperty("waveCount")]
  public long WaveCount { get; set; }

  [JsonProperty("puzzlesPerWave")]
  public long PuzzlesPerWave { get; set; }
}

public partial class StageDefinitions {
  public static StageDefinitions FromJson(string json) => JsonConvert.DeserializeObject<StageDefinitions>(json, Converter.Settings);
}

public static class Serialize {
  public static string ToJson(this StageDefinitions self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter {
  public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    DateParseHandling = DateParseHandling.None,
    Converters =
      {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
  };
}
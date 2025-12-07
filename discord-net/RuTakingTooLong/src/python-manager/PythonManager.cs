using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace HoloSimpID;

public static partial class Python {
  private static readonly HttpClient client_ = new();
  private static readonly string url_base_ =
      $"http://python-uvicorn:{Environment.GetEnvironmentVariable("UVICORN_PORT")}/";

  public static async Task<(bool success, string value)> Invoke(
      string fast_api_key, JsonObject parameters) {
    string result;
    try {
      var response =
          await client_.PostAsync(url_base_ + fast_api_key, JsonContent.Create(parameters));
      response.EnsureSuccessStatusCode();
      result = await response.Content.ReadAsStringAsync();
      return (true, result);
    } catch (Exception ex) {
      Console.WriteLine(ex.ToStringDemystified());
      return (false, $"Error calling {fast_api_key}");
    }
  }
}

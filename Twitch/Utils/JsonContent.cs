using System.Net.Http;
using System.Text;

namespace Twitch.Utils;

internal class JsonContent(string jsonText) : StringContent(jsonText, Encoding.UTF8, "application/json") { }
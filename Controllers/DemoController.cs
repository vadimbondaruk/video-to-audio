using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace video_to_audio.Controllers
{
  [Route("demo")]
  public class DemoController : Controller
  {

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadAndConvert()
    {
      var boundary = this.Request.ContentType.Substring(this.Request.ContentType.LastIndexOf('=') + 1);

      var reader = new MultipartReader(boundary, Request.Body);

      var section = await reader.ReadNextSectionAsync();

      var fileName = section.GetContentDispositionHeader().FileName.Value;

      using var fileStream = new FileStream(fileName, FileMode.Create);
      await section.Body.CopyToAsync(fileStream);

      var outputAudioFilePath = string.Concat(Path.GetFileNameWithoutExtension(fileName), ".mp3");

      Process process = new Process();
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.FileName = "/usr/bin/ffmpeg";

      process.StartInfo.Arguments = $"-i {fileName} -codec:a libmp3lame {outputAudioFilePath}";


      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.Start();
      await process.WaitForExitAsync();

      return Ok();
    }
  }
}
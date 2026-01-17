using System.Net;
using Dto.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Model;

namespace ChuanLeMaServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _fileDir;

        public FileController(ILogger<FileController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _fileDir = _configuration["FileDir"];
        }

        /// <summary>
        /// 获取工作目录下的文件和文件夹列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("FileDirList")]
        public IActionResult FileList()
        {
            _logger.LogInformation("FileList 请求");
            DirectoryInfo dirinfo = new DirectoryInfo(_fileDir);
            FileInfo[] files = dirinfo.GetFiles();
            DirectoryInfo[] dirs = dirinfo.GetDirectories();
            List<FolderFileDataModel> folderfiles = new List<FolderFileDataModel>();
            foreach (FileInfo file in files)
            {
                folderfiles.Add(new FolderFileDataModel()
                {
                    Name = file.Name,
                    IsFolder = false,
                    Size = file.Length,
                    Tags = new List<TagInfo>()
                    {
                        new TagInfo { Name = "文件", Color = "geekblue" }
                    }
                });
            }

            foreach (DirectoryInfo dir in dirs)
            {
                folderfiles.Add(new FolderFileDataModel()
                {
                    Name = dir.Name,
                    IsFolder = false,
                    Size = 0,
                    Tags = new List<TagInfo>()
                    {
                        new TagInfo { Name = "目录", Color = "geek" }
                    }
                });
            }

            return Ok(new ResponseResult<List<FolderFileDataModel>>(folderfiles));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downloadRequestDto"></param>
        /// <returns></returns>
        [HttpPost("downloadfile")]
        public IActionResult DownloadFile(FileDownloadRequestDto downloadRequestDto)
        {
            string fullpath = Path.Combine(_fileDir, downloadRequestDto.filepath);
            FileStream fs = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            // 启用分块传输，适合大文件
            Response.Headers.Add("Accept-Ranges", "bytes");

            FileInfo fileInfo = new FileInfo(fullpath);
            IContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();
            // 尝试获取 MIME 类型
            string mimetype = "";
            if (_contentTypeProvider.TryGetContentType(fileInfo.FullName, out string contentType))
            {
                mimetype = contentType;
            }
            else
            {
                mimetype = "application/octet-stream";
            }
            return new FileStreamResult(fs, mimetype)
            {
                FileDownloadName = fileInfo.Name,
                EnableRangeProcessing = true // 支持断点续传
            };
        }
    }
}
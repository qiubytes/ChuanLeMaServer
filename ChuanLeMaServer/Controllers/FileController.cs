using Dto.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            return Ok(folderfiles);
        }
    }
}
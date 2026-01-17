using Microsoft.AspNetCore.Http;

namespace Dto.File;

public class FileUploadDto
{
    /// <summary>
    /// 上传到的工作目录
    /// </summary>
    public string workpath { get; set; } 
    /// <summary>
    /// 上传文件
    /// </summary>
    public IFormFile File { get; set; }
}
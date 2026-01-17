namespace Model;

public class FolderFileDataModel
{
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }

    /// <summary>
    /// 是否是文件夹 否则是文件
    /// </summary>
    public bool IsFolder { get; set; }

    public List<TagInfo> Tags { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}
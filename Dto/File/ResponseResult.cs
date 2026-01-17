namespace Dto.File;

/// <summary>
/// 泛型 控制器返回类
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResponseResult<T>
{
    public ResponseResult(T tData)
    {
        data = tData;
    }

    /// <summary>
    /// 状态码
    /// </summary>
    public int code { get; set; } = 200;

    /// <summary>
    /// 消息
    /// </summary>
    public string msg { get; set; } = "";
    /// <summary>
    /// 数据
    /// </summary>
    public T data { get; set; }
}
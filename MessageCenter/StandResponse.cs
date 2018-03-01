namespace MessageCenter
{
    /// <summary>
    /// 统一返回结果
    /// </summary>
    public class StandResponse
    {
        /// <summary>
        /// 返回代码：-1表示系统错误，0=表示成功，=1表示业务错误，需要参考Desc的错误描述
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 当有业务数据检查错误时，返回错误描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 如果有返回数据，返回数据将以Json格式序列化在Data中
        /// </summary>
        public string Data { get; set; }
    }
}
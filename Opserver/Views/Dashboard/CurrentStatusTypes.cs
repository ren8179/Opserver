using System.ComponentModel;

namespace StackExchange.Opserver.Views.Dashboard
{
    public enum CurrentStatusTypes
    {
        [Description("无")]
        None = 0,
        [Description("状态")]
        Stats = 1,
        [Description("接口")]
        Interfaces = 2,
        [Description("VM Info")]
        VMHost = 3,
        [Description("Elastic")]
        Elastic = 4,
        [Description("HAProxy")]
        HAProxy = 5,
        [Description("数据库实例")]
        SQLInstance = 6,
        [Description("活动 SQL")]
        SQLActive = 7,
        [Description("热门 SQL")]
        SQLTop = 8,
        [Description("Redis Info")]
        Redis = 9
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace StackExchange.Opserver.Helpers
{
    /// <summary>
    /// Redis 帮助类
    /// </summary>
    public static class RedisHelper
    {
        private static string _conn = "127.0.0.1:6379";
        static ConnectionMultiplexer _redis;
        static readonly object _locker = new object();
        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;
                        _redis = GetManager();
                        return _redis;
                    }
                }
                return _redis;
            }
        }
        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = _conn;
            }
            var options = ConfigurationOptions.Parse(connectionString);
            return ConnectionMultiplexer.Connect(options);

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireMinutes">过期时间，单位：分钟。默认600分钟</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        /// <returns></returns>
        public static bool StringSet(string key, string value, int expireMinutes = 600, int db = -1)
        {
            return Manager.GetDatabase(db).StringSet(key, value, TimeSpan.FromMinutes(expireMinutes));
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        /// <returns></returns>
        public static string StringGet(string key, int db = -1)
        {
            try
            {
                return Manager.GetDatabase(db).StringGet(key);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        /// <returns></returns>
        public static bool StringRemove(string key, int db = -1)
        {
            try
            {
                return Manager.GetDatabase(db).KeyDelete(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        public static bool KeyExists(string key, int db = -1)
        {
            try
            {
                return Manager.GetDatabase(db).KeyExists(key);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 延期
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="min">延长时间，单位：分钟，默认600分钟</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        public static bool AddExpire(string key, int min = 600, int db = -1)
        {
            try
            {
                return Manager.GetDatabase(db).KeyExpire(key, DateTime.Now.AddMinutes(min));
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="t">实体</param>
        /// <param name="ts">延长时间，单位：分钟，默认600分钟</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        public static bool Set<T>(string key, T t, int expireMinutes = 600, int db = -1)
        {
            try
            {
                var str = JsonConvert.SerializeObject(t,new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });
                return Manager.GetDatabase(db).StringSet(key, str, TimeSpan.FromMinutes(expireMinutes));
            }
            catch(Exception ex)
            {
                throw new Exception("Redis缓存实体出错", ex);
            }
        }
        
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="db">库，默认第一个。0~15共16个库</param>
        public static T Get<T>(string key, int db = -1) where T : class
        {
            var strValue = Manager.GetDatabase(db).StringGet(key);
            return string.IsNullOrEmpty(strValue) ? null : JsonConvert.DeserializeObject<T>(strValue);
        }
    }
}

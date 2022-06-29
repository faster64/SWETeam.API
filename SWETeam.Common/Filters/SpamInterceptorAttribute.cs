﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SWETeam.Common.Caching;
using SWETeam.Common.Properties;
using System;

namespace SWETeam.Common.Filters
{
    public class SpamInterceptorAttribute : Attribute, IResourceFilter
    {
        private readonly int _maxRequest;
        private readonly int _spamTime;
        private readonly double _blockTime;
        public string path;
        public string ip;

        public SpamInterceptorAttribute(int maxRequest, int spamTime, double blockTime)
        {
            _maxRequest = maxRequest;
            _spamTime = spamTime;
            _blockTime = blockTime;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            ip = context.HttpContext.Request.Headers["X-Forwarded-For"];
            path = context.HttpContext.Request.Path;

            if (ip == null)
            {
                ip = "0.0.0.0";
            }

            // Check xem IP có đang bị block không?
            if (CheckBlocked())
            {
                var blockedTime = GetBlockedTime() ?? DateTime.Now;
                var seconds = (int)(_blockTime * 60) - (int)(DateTime.Now - blockedTime).TotalSeconds;
                var minutes = (int)Math.Floor(1.0 * seconds / 60);

                context.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = string.Format(Resources.SpamRemainderMessage, ip, minutes, seconds - (minutes * 60)),
                    ContentType = "application/json"
                };
                return;
            }

            // Check xem IP có đang spam không?
            var requestCount = GetRequestCount();
            if (requestCount == null)
            {
                SetRequestCount(1);
                return;
            }

            SetRequestCount((int)requestCount + 1);

            // Nếu tổng request >= maxRequest thì block IP;
            if ((int)requestCount >= _maxRequest)
            {
                BlockedIP();
                context.Result = new ContentResult()
                {
                    StatusCode = 400,
                    Content = String.Format(Resources.SpamMessage, ip, _blockTime),
                    ContentType = "application/json"
                };
                return;
            }
        }

        private DateTime? GetBlockedTime()
        {
            var cache = MemoryCacheHelper.Get($"{path}_{ip}_blocked");
            if (cache == null)
            {
                return null;
            }
            return DateTime.Parse(cache.ToString());
        }

        /// <summary>
        /// Get request count
        /// </summary>
        private bool CheckBlocked()
        {
            return GetBlockedTime() != null;
        }

        /// <summary>
        /// Get request count
        /// </summary>
        private object GetRequestCount()
        {
            return MemoryCacheHelper.Get($"{path}_{ip}_request_count");
        }

        /// <summary>
        /// Set request count
        /// </summary>
        private void SetRequestCount(int count)
        {
            MemoryCacheHelper.Set($"{path}_{ip}_request_count", count, (1.0 * _spamTime) / _maxRequest);
        }

        /// <summary>
        /// Block IP
        /// </summary>
        private void BlockedIP()
        {
            MemoryCacheHelper.Set($"{path}_{ip}_blocked", DateTime.Now, _blockTime);
            MemoryCacheHelper.Remove($"{path}_{ip}_request_count");
        }

    }
}


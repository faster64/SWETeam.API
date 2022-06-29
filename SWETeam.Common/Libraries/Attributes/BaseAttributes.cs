using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Libraries
{
    public class BaseAttributes
    {
    }

    /// <summary>
    /// Flag Required yêu cầu không được để trống
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Required : Attribute
    {
    }

    /// <summary>
    /// Flag Duplication yêu cầu thông tin không được trùng 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Duplication : Attribute
    {
    }

    /// <summary>
    /// Khóa chính
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Key : Attribute
    {
    }

    /// <summary>
    /// Ignore
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Ignore : Attribute
    {
    }

    /// <summary>
    /// PhoneNumber
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PhoneNumber: Attribute
    {
    }

    /// <summary>
    /// Email
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Email : Attribute
    {
    }

    /// <summary>
    /// Password
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Password : Attribute
    {
    }
}

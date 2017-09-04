using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace ZhiDiShuJuWepAPI
{
    public class ExcludeEntityKeyContractResolver : DefaultContractResolver
    {
        protected override List<System.Reflection.MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (objectType.Namespace != null && objectType.Namespace.StartsWith("System.Data.Entity.Dynamic"))
            {
                return base.GetSerializableMembers(objectType.BaseType);
            }

            return base.GetSerializableMembers(objectType);
        } 
    }
}
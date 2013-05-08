using System;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using MVCPresentation.Web.Models;

namespace MVCPresentation.Web.Persistence
{
    /// <summary>
    ///   The main auto-mapping class for FNH.
    /// </summary>
    public class AppAutomappingCfg : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            if (type.ContainsGenericParameters)
                return false;

            if (type.IsEnum || type.IsNested || type.IsInterface)
                return false;

            return IsEntity(type);
        }

        public static bool IsEntity(Type type)
        {
            return typeof (IEntity).IsAssignableFrom(type);
        }

        public override bool AbstractClassIsLayerSupertype(Type type)
        {
            return IsEntity(type) == false || type.IsAbstract == true;
        }

        // ReSharper disable RedundantOverridenMember
        public override bool ShouldMap(Member member)
        {
            if (base.ShouldMap(member) == false)
                return false;

            if (member.IsProperty)
            {
                var property = (PropertyInfo) member.MemberInfo;

                // no setter, no mapping
                if (property.GetSetMethod(true) == null)
                    return false;
            }

            return true;
        }
    }
}
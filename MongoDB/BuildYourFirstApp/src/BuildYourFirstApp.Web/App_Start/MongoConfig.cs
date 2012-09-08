using BuildYourFirstApp.Web.Data;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace BuildYourFirstApp.Web
{
    public class MongoConfig
    {
        public static void Initialize()
        {
            var profile = new ConventionProfile();
            profile.SetElementNameConvention(new LowerCaseUnderscoreElementNameConvention());
            BsonClassMap.RegisterConventions(profile, x => true);
        }

        private class LowerCaseUnderscoreElementNameConvention : IElementNameConvention
        {
            private static readonly Regex _regex = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            public string GetElementName(MemberInfo member)
            {
                return _regex.Replace(member.Name, "_").ToLower();
            }
        }

    }
}
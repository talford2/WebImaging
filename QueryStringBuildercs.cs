using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataway.WebImaging
{
    public class QueryStringBuilder
    {
        #region Private Members

        private List<QueryStringValuePair> parts;

        #endregion

        #region Constructors

        public QueryStringBuilder()
        {
            parts = new List<QueryStringValuePair>();
        }

        public QueryStringBuilder(string url)
            : this()
        {
            this.Url = url;
        }

        #endregion

        #region Public Properties

        public string Url { get; set; }

        #endregion

        #region Public Methods

        public void Add(string name, object value)
        {
            parts.Add(new QueryStringValuePair(name, value));
        }

        public void RemoveAt(int index)
        {
            parts.RemoveAt(index);
        }

        public void Remove(string name)
        {
            var remove = parts.First(p => p.Name == name);
            parts.Remove(remove);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Url);

            if (this.parts.Any())
            {
				if (this.Url.Contains("?"))
				{
					sb.Append("&");
				}
				else
				{
					sb.Append("?");
				}
            }
            var cleanedParts = parts.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToList();
            foreach (var part in cleanedParts)
            {
                sb.AppendFormat("{0}={1}&", part.Name, part.Value);
            }
            return sb.ToString().TrimEnd("&".ToCharArray());
        }

        #endregion
    }

    public class QueryStringValuePair
    {
        public QueryStringValuePair(string name, object value)
        {
            this.Name = name;
            if (value != null)
            {
                this.Value = value.ToString();
            }
            else
            {
                this.Value = null;
            }
        }

        public string Name { get; set; }

        public string Value { get; set; }

    }
}

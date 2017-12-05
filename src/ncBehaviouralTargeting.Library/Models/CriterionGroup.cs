using System;
using System.Collections.Generic;
using System.Linq;
using ncBehaviouralTargeting.Library.Models.BaseModels;
using NPoco;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName("ncBtCriterionGroup")]
    internal class CriterionGroup : BaseEntity<CriterionGroup>
    {
        public int SegmentId { get; set; }
        public bool IsInclude { get; set; }
        public int SortOrder { get; set; }

        [Ignore]
        public List<Criterion> Criterions { get; set; }



        public void SetLinks(List<Criterion> criterions, List<Operator> operators)
        {
            Criterions = (criterions ?? new List<Criterion>()).Where(x => x.CriterionGroupId == Id).ToList();
            Criterions.ForEach(criterion => criterion.SetLinks(operators));
        }

        public static CriterionGroup GetById(int id)
        {
            using (var connection = CreateConnection)
            {
                var tuple = connection.FetchMultiple<CriterionGroup, Criterion, Operator>(";exec CriterionGroupGetByIdStp @Id", new { Id = id });
                var item = tuple.Item1.FirstOrDefault();
                if (item == null)
                {
                    return null;
                }
                item.SetLinks(tuple.Item2, tuple.Item3);
                return item;
            }
        }

        public bool Match(IReadOnlyDictionary<string, object> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (Criterions == null || Criterions.Count == 0)
            {
                return true;
            }

            return (from criterion in Criterions
                    where properties.ContainsKey(criterion.PropertyAlias)
                    let actual = properties[criterion.PropertyAlias]
                    where criterion.Match(actual)
                    select criterion).Any();
        }
    }
}

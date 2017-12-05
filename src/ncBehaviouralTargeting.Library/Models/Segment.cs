using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ncBehaviouralTargeting.Library.Models.BaseModels;
using NPoco;
using NPoco.Expressions;

namespace ncBehaviouralTargeting.Library.Models
{
    [TableName("ncBtSegment")]
    [DebuggerDisplay("{DisplayName}, Alias = {Alias}, HasCriterions = {HasCriterions}")]
    internal class Segment : BaseEntity<Segment>
    {
        public string Alias { get; set; }
        public string DisplayName { get; set; }
        public int Persistence { get; set; }

        [Ignore]
        public List<CriterionGroup> CriterionGroups { get; set; }
        [Ignore]
        public bool HasCriterions { get { return CriterionGroups.Any(cg => cg.Criterions.Any()); } }

        internal Segment()
        {
            CriterionGroups = new List<CriterionGroup>();
        }

        public void SetLinks(List<CriterionGroup> criterionGroups, List<Criterion> criterions, List<Operator> operators)
        {
            CriterionGroups = (criterionGroups ?? new List<CriterionGroup>())
                .Where(x => x.SegmentId == Id)
                .ToList();
            CriterionGroups.ForEach(criterionGroup => criterionGroup
                .SetLinks(criterions, operators));
        }

        private bool SaveSegment(Database currentConnection, Segment segment)
        {
            // Begin transaction
            currentConnection.BeginTransaction();

            try
            {
                // Save segment
                currentConnection.Save<Segment>(segment);

                // Get lists of new and updated criterion groups
                var insertCriterionGroups = segment.CriterionGroups
                    .Where(x => x.Id == 0)
                    .ToList();
                var updateCriterionGroups = segment.CriterionGroups
                    .Where(x => x.Id != 0)
                    .ToList();
                var updateCriterionGroupIds = updateCriterionGroups
                    .Select(x => x.Id)
                    .ToList();

                // Get lists of new and updated criterions
                var insertCriterions = new List<Criterion>();
                segment.CriterionGroups
                    .ForEach(grp =>
                        grp.Criterions
                            .Where(cri => cri.Id == 0)
                            .ToList()
                            .ForEach(cri => insertCriterions.Add(cri)));
                var updateCriterions = new List<Criterion>();
                segment.CriterionGroups
                    .ForEach(grp =>
                        grp.Criterions
                            .Where(cri => cri.Id != 0)
                            .ToList()
                            .ForEach(cri => updateCriterions.Add(cri)));
                var updateCriterionIds = updateCriterions
                    .Select(x => x.Id)
                    .ToList();

                // Delete removed criteria groups
                currentConnection.DeleteMany<CriterionGroup>().Where(x => x.SegmentId == segment.Id && !x.Id.In(updateCriterionGroupIds)).Execute();
                // Update current criteria groups
                updateCriterionGroups.ForEach(currentConnection.Save<CriterionGroup>);
                // Insert new criteria groups, cant use bulk as we need the new ids
                insertCriterionGroups.ForEach(currentConnection.Save<CriterionGroup>);

                // Add ids for newly created criteria groups to newly created criterions in them
                insertCriterionGroups.ForEach(x => x.Criterions.ForEach(y => y.CriterionGroupId = x.Id));

                // Delete removed criteria
                currentConnection.DeleteMany<Criterion>().Where(x => x.CriterionGroupId.In(updateCriterionGroupIds) && !x.Id.In(updateCriterionIds)).Execute();
                // Update current criteria
                updateCriterions.ForEach(currentConnection.Save<Criterion>);
                // Insert new criteria
                currentConnection.InsertBulk(insertCriterions);

                // Success!
                currentConnection.CompleteTransaction();
                return true;
            }
            catch
            {
                // Something went wrong, rollback
                currentConnection.AbortTransaction();
                return false;
            }
        }

        public static Segment GetById(int id)
        {
            using (var connection = CreateConnection)
            {
                var tuple = connection.FetchMultiple<Segment, CriterionGroup, Criterion, Operator>(";exec SegmentGetByIdStp @Id", new { Id = id });
                var item = tuple.Item1.FirstOrDefault();
                if (item == null)
                {
                    return null;
                }
                item.SetLinks(tuple.Item2, tuple.Item3, tuple.Item4);
                return item;
            }
        }

        public static Segment GetLightById(int id)
        {
            using (var connection = CreateConnection)
            {
                return connection.SingleOrDefaultById<Segment>(id);
            }
        }

        public static Segment GetByAlias(string alias)
        {
            using (var connection = CreateConnection)
            {
                var tuple = connection.FetchMultiple<Segment, CriterionGroup, Criterion, Operator>(";exec SegmentGetByAliasStp @Alias", new { Alias = alias });
                var item = tuple.Item1.FirstOrDefault();
                if (item == null)
                {
                    return null;
                }
                item.SetLinks(tuple.Item2, tuple.Item3, tuple.Item4);
                return item;
            }
        }

        public static List<Segment> GetAll()
        {
            using (var connection = CreateConnection)
            {
                var tuple = connection.FetchMultiple<Segment, CriterionGroup, Criterion, Operator>(";exec SegmentGetAllStp");
                var segments = tuple.Item1;
                segments.ForEach(s => s.SetLinks(tuple.Item2, tuple.Item3, tuple.Item4));
                return segments;
            }
        }

        public new bool Save(Database currentConnection = null)
        {
            if (currentConnection == null)
            {
                using (var connection = CreateConnection)
                {
                    return SaveSegment(connection, this);
                }
            }
            else
            {
                return SaveSegment(currentConnection, this);
            }
        }

        public bool Match(IReadOnlyDictionary<string, object> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            return CriterionGroups.Aggregate(true, (current, criterionGroup) => current && criterionGroup.Match(properties));
        }
    }
}

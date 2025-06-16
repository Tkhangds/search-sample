using Eme_Search.Common;

namespace Eme_Search.Database.Models;

public class BlacklistCategory: BaseEntity
{
    public int Id { get; set; }
    public string Alias { get; set; }
    public string Title { get; set; }
}
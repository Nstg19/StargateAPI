using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("Log")]
public class Log
{
	public int Id { get; set; }
	public string Type { get; set; } = string.Empty;
	public string Details { get; set; } = string.Empty;
	public DateTime CreatedDateTime { get; set; }
}

public class LogConfiguration : IEntityTypeConfiguration<Log>
{
	public void Configure(EntityTypeBuilder<Log> builder)
	{
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).ValueGeneratedOnAdd();
	}
}

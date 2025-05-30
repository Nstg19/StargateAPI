﻿namespace StargateAPI.Business.Dtos;

public class PersonAstronautDuty
{
	public int Id { get; set; }

	public string Rank { get; set; } = string.Empty;

	public string DutyTitle { get; set; } = string.Empty;

	public DateTime DutyStartDate { get; set; }

	public DateTime? DutyEndDate { get; set; }
}

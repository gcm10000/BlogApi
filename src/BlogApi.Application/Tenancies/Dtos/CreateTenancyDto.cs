﻿namespace BlogApi.Application.Tenancies.Dtos;

public class CreateTenancyDto : TenancyDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

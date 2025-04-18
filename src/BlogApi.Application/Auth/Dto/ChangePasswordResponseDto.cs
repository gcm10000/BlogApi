﻿namespace BlogApi.Application.Auth.Dto;

public class ChangePasswordResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = [];
}
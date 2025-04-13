﻿using MediatR;

namespace BlogApi.Application.Posts.Commands.DeleteCategory;

// Command
public class DeleteCategoryCommand : IRequest<bool>
{
    public string Name { get; set; }
}

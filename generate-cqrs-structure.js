const fs = require('fs');
const path = require('path');

const root = './CQRS_Blog';

const structure = [
  'Blog.Domain/Entities',
  'Blog.Application/DTOs',
  'Blog.Application/Posts/Commands/CreatePost',
  'Blog.Application/Posts/Queries/GetPostById',
  'Blog.API/Controllers',
  'Blog.Infrastructure/Data'
];

const files = {
  'Blog.Domain/Entities/Post.cs': `
namespace Blog.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public List<string> Categories { get; set; } = new();
    public string Status { get; set; } = "draft";
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
`,

  'Blog.Application/DTOs/PostDto.cs': `
namespace Blog.Application.DTOs;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
`,

  'Blog.Application/Posts/Commands/CreatePost/CreatePostCommand.cs': `
using MediatR;
using Blog.Application.DTOs;

namespace Blog.Application.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<PostDto>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public List<string> Categories { get; set; } = new();
    public string Status { get; set; } = "draft";
}
`,

  'Blog.Application/Posts/Commands/CreatePost/CreatePostCommandHandler.cs': `
using MediatR;
using Blog.Domain.Entities;
using Blog.Application.DTOs;
using Blog.Infrastructure.Data;

namespace Blog.Application.Posts.Commands.CreatePost;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly BlogDbContext _context;

    public CreatePostCommandHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            Excerpt = request.Excerpt,
            Image = request.Image,
            AuthorId = request.AuthorId,
            Categories = request.Categories,
            Status = request.Status,
            Date = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Excerpt = post.Excerpt,
            Image = post.Image,
            Status = post.Status,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
`,

  'Blog.Application/Posts/Queries/GetPostById/GetPostByIdQuery.cs': `
using MediatR;
using Blog.Application.DTOs;

namespace Blog.Application.Posts.Queries.GetPostById;

public class GetPostByIdQuery : IRequest<PostDto?>
{
    public int Id { get; set; }

    public GetPostByIdQuery(int id)
    {
        Id = id;
    }
}
`,

  'Blog.Application/Posts/Queries/GetPostById/GetPostByIdQueryHandler.cs': `
using MediatR;
using Microsoft.EntityFrameworkCore;
using Blog.Application.DTOs;
using Blog.Infrastructure.Data;

namespace Blog.Application.Posts.Queries.GetPostById;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto?>
{
    private readonly BlogDbContext _context;

    public GetPostByIdQueryHandler(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (post == null) return null;

        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Excerpt = post.Excerpt,
            Image = post.Image,
            Status = post.Status,
            Date = post.Date,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}
`,

  'Blog.Infrastructure/Data/BlogDbContext.cs': `
using Microsoft.EntityFrameworkCore;
using Blog.Domain.Entities;

namespace Blog.Infrastructure.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

    public DbSet<Post> Posts => Set<Post>();
}
`,

  'Blog.API/Controllers/PostsController.cs': `
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Blog.Application.Posts.Commands.CreatePost;
using Blog.Application.Posts.Queries.GetPostById;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetPostByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
`
};

function ensureDirSync(dirPath) {
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
}

function writeStructure() {
  structure.forEach(folder => {
    const dir = path.join(root, folder);
    ensureDirSync(dir);
  });

  for (const [filePath, content] of Object.entries(files)) {
    const fullPath = path.join(root, filePath);
    fs.writeFileSync(fullPath, content.trimStart());
  }

  console.log('✅ Estrutura CQRS com MediatR e EF Core gerada com sucesso!');
}

writeStructure();

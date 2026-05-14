using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MarketingIntelligence.Shared.Results;
using MarketingIntelligence.Modules.SocialMedia.Core.Interfaces;
using MarketingIntelligence.Modules.SocialMedia.Core.Entities;

namespace MarketingIntelligence.Modules.SocialMedia.Core.UseCases.Posts;

public class SchedulePostCommandHandler(IPostRepository postRepository) 
    : IRequestHandler<SchedulePostCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SchedulePostCommand request, CancellationToken cancellationToken)
    {
        // Validação de negócio básica
        if (request.ScheduledAt <= DateTimeOffset.UtcNow)
        {
            return Result<Guid>.Failure("A data de agendamento deve estar no futuro.");
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<Guid>.Failure("O conteúdo da postagem não pode ser vazio.");
        }

        // Criação da entidade (Aggregate Root)
        var post = new Post(request.BrandId, request.Content, request.ScheduledAt, request.Platforms);

        // Persistência
        await postRepository.AddAsync(post, cancellationToken);
        await postRepository.SaveChangesAsync(cancellationToken);

        // Retorno de sucesso sem lançar exceções
        return Result<Guid>.Success(post.Id);
    }
}
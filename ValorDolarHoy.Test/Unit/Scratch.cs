using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Observable.Aliases;
using Xunit;

namespace ValorDolarHoy.Test.Unit;

public class Scratch
{
    [Fact]
    public void Observable_Ok()
    {
        RecommendedItemsDto recommendedItemsDto = GetRecommmendations()
            .FlatMap(recommmendationsDto => recommmendationsDto.Values)
            .FlatMap(itemId => GetItemById(itemId)
                .Map(itemDto =>
                {
                    RecommendedItemDto recommendedItemDto = new(itemDto);
                    return recommendedItemDto;
                }))
            .ToList()
            .Map(recommendedItemDtos =>
            {
                RecommendedItemsDto recommendedItemsDto = new(recommendedItemDtos);
                return recommendedItemsDto;
            })
            .Wait();

        Assert.NotNull(recommendedItemsDto);
        Assert.NotNull(recommendedItemsDto.Items);
        Assert.Equal(2, recommendedItemsDto.Items.Count());
        Assert.Equal("Test item 1", recommendedItemsDto.Items.Skip(0).Take(1).First().ItemDto.Title);
        Assert.Equal("Test item 2", recommendedItemsDto.Items.Skip(1).Take(1).First().ItemDto.Title);
    }

    /*
     * Task 1
     */
    private static IObservable<RecommmendationsDto> GetRecommmendations()
    {
        RecommmendationsDto recommmendationsDto = new(new List<string>
        {
            "MLA1", "MLA2"
        });

        return Observable.Return(recommmendationsDto);
    }

    /*
     * Task 2
     */
    private static IObservable<ItemDto> GetItemById(string itemId)
    {
        ItemDto itemDto1 = new("MLA1", "Test item 1");
        ItemDto itemDto2 = new("MLA2", "Test item 2");

        Dictionary<string, ItemDto> itemDtos = new()
        {
            { itemDto1.Id, itemDto1 },
            { itemDto2.Id, itemDto2 }
        };

        ItemDto itemDto = itemDtos[itemId];

        return Observable.Return(itemDto);
    }
}

internal class RecommmendationsDto
{
    public RecommmendationsDto(List<string> values)
    {
        this.Values = values;
    }

    public List<string> Values { get; }
}

internal class ItemDto
{
    public ItemDto(string id, string title)
    {
        this.Id = id;
        this.Title = title;
    }

    public string Id { get; }
    public string Title { get; }
}

internal class RecommendedItemsDto
{
    public RecommendedItemsDto(IEnumerable<RecommendedItemDto> items)
    {
        this.Items = items;
    }

    public IEnumerable<RecommendedItemDto> Items { get; }
}

internal class RecommendedItemDto
{
    public RecommendedItemDto(ItemDto itemDto)
    {
        this.ItemDto = itemDto;
    }

    public ItemDto ItemDto { get; }
}
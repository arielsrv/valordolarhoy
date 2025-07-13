export class CurrencyDto {
    official: OfficialDto = new OfficialDto();
    blue: BlueDto = new BlueDto();
}

export class OfficialDto {
    buy!: number;
    sell!: number;
}

export class BlueDto {
    buy!: number;
    sell!: number;
}

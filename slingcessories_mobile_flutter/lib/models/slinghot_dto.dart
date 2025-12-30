class SlinghotDto {
  final int id;
  final int year;
  final String model;
  final String color;

  SlinghotDto({
    required this.id,
    required this.year,
    required this.model,
    required this.color,
  });

  factory SlinghotDto.fromJson(Map<String, dynamic> json) {
    return SlinghotDto(
      id: json['id'],
      year: json['year'],
      model: json['model'],
      color: json['color'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'year': year,
      'model': model,
      'color': color,
    };
  }

  String get displayName => '$year $model ($color)';
}


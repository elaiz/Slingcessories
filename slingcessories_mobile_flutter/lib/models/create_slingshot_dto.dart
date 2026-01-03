class CreateSlingshotDto {
  final int year;
  final String model;
  final String color;
  final String userId;

  CreateSlingshotDto({
    required this.year,
    required this.model,
    required this.color,
    required this.userId,
  });

  Map<String, dynamic> toJson() {
    return {
      'year': year,
      'model': model,
      'color': color,
      'userId': userId,
    };
  }
}


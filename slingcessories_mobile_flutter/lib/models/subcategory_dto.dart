class SubcategoryDto {
  final int id;
  final String name;
  final int categoryId;

  SubcategoryDto({
    required this.id,
    required this.name,
    required this.categoryId,
  });

  factory SubcategoryDto.fromJson(Map<String, dynamic> json) {
    return SubcategoryDto(
      id: json['id'],
      name: json['name'],
      categoryId: json['categoryId'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'categoryId': categoryId,
    };
  }
}


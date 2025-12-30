class CreateSubcategoryDto {
  final String name;
  final int categoryId;

  CreateSubcategoryDto({required this.name, required this.categoryId});

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'categoryId': categoryId,
    };
  }
}


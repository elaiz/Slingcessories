class CreateCategoryDto {
  final String name;

  CreateCategoryDto({required this.name});

  Map<String, dynamic> toJson() {
    return {'name': name};
  }
}


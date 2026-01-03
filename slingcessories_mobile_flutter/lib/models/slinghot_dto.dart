class SlinghotDto {
  final int id;
  final int year;
  final String model;
  final String color;
  final double totalOwned;
  final double totalWishlist;
  final double totalAccessories;
  final List<SlingshotAccessoryGroup>? ownedAccessories;
  final List<SlingshotAccessoryGroup>? wishlistAccessories;

  SlinghotDto({
    required this.id,
    required this.year,
    required this.model,
    required this.color,
    this.totalOwned = 0,
    this.totalWishlist = 0,
    this.totalAccessories = 0,
    this.ownedAccessories,
    this.wishlistAccessories,
  });

  factory SlinghotDto.fromJson(Map<String, dynamic> json) {
    return SlinghotDto(
      id: json['id'],
      year: json['year'],
      model: json['model'],
      color: json['color'],
      totalOwned: (json['totalOwned'] as num?)?.toDouble() ?? 0,
      totalWishlist: (json['totalWishlist'] as num?)?.toDouble() ?? 0,
      totalAccessories: (json['totalAccessories'] as num?)?.toDouble() ?? 0,
      ownedAccessories: json['ownedAccessories'] != null
          ? (json['ownedAccessories'] as List)
              .map((e) => SlingshotAccessoryGroup.fromJson(e))
              .toList()
          : null,
      wishlistAccessories: json['wishlistAccessories'] != null
          ? (json['wishlistAccessories'] as List)
              .map((e) => SlingshotAccessoryGroup.fromJson(e))
              .toList()
          : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'year': year,
      'model': model,
      'color': color,
      'totalOwned': totalOwned,
      'totalWishlist': totalWishlist,
      'totalAccessories': totalAccessories,
    };
  }

  String get displayName => '$year $model';
}

class SlingshotAccessoryGroup {
  final String categoryName;
  final int count;
  final List<SlingshotAccessoryItem>? items;

  SlingshotAccessoryGroup({
    required this.categoryName,
    required this.count,
    this.items,
  });

  factory SlingshotAccessoryGroup.fromJson(Map<String, dynamic> json) {
    return SlingshotAccessoryGroup(
      categoryName: json['categoryName'] ?? json['name'] ?? '',
      count: json['count'] ?? 0,
      items: json['items'] != null
          ? (json['items'] as List)
              .map((e) => SlingshotAccessoryItem.fromJson(e))
              .toList()
          : null,
    );
  }
}

class SlingshotAccessoryItem {
  final int id;
  final String title;
  final double price;

  SlingshotAccessoryItem({
    required this.id,
    required this.title,
    required this.price,
  });

  factory SlingshotAccessoryItem.fromJson(Map<String, dynamic> json) {
    return SlingshotAccessoryItem(
      id: json['id'],
      title: json['title'] ?? '',
      price: (json['price'] as num?)?.toDouble() ?? 0,
    );
  }
}

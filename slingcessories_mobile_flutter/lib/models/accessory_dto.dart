class AccessoryDto {
  final int id;
  final String title;
  final String? pictureUrl;
  final double price;
  final String? url;
  final bool wishlist;
  final int categoryId;
  final int? subcategoryId;
  final String categoryName;
  final String? subcategoryName;
  final List<int>? slinghotIds;
  final List<String>? slinghotDescriptions;
  final Map<String, int> slinghotQuantities;

  AccessoryDto({
    required this.id,
    required this.title,
    this.pictureUrl,
    required this.price,
    this.url,
    required this.wishlist,
    required this.categoryId,
    this.subcategoryId,
    required this.categoryName,
    this.subcategoryName,
    this.slinghotIds,
    this.slinghotDescriptions,
    required this.slinghotQuantities,
  });

  factory AccessoryDto.fromJson(Map<String, dynamic> json) {
    return AccessoryDto(
      id: json['id'],
      title: json['title'],
      pictureUrl: json['pictureUrl'],
      price: (json['price'] as num).toDouble(),
      url: json['url'],
      wishlist: json['wishlist'],
      categoryId: json['categoryId'],
      subcategoryId: json['subcategoryId'],
      categoryName: json['categoryName'],
      subcategoryName: json['subcategoryName'],
      slinghotIds: json['slinghotIds'] != null 
          ? List<int>.from(json['slinghotIds']) 
          : null,
      slinghotDescriptions: json['slinghotDescriptions'] != null
          ? List<String>.from(json['slinghotDescriptions'])
          : null,
      slinghotQuantities: json['slinghotQuantities'] != null
          ? Map<String, int>.from(
              json['slinghotQuantities'].map((k, v) => MapEntry(k.toString(), v as int)))
          : {},
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'pictureUrl': pictureUrl,
      'price': price,
      'url': url,
      'wishlist': wishlist,
      'categoryId': categoryId,
      'subcategoryId': subcategoryId,
      'categoryName': categoryName,
      'subcategoryName': subcategoryName,
      'slinghotIds': slinghotIds,
      'slinghotDescriptions': slinghotDescriptions,
      'slinghotQuantities': slinghotQuantities,
    };
  }
}


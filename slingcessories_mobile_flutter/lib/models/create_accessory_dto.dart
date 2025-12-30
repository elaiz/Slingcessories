class CreateAccessoryDto {
  final String title;
  final String? pictureUrl;
  final double price;
  final String? url;
  final bool wishlist;
  final int categoryId;
  final int? subcategoryId;
  final Map<String, int> slinghotQuantities;

  CreateAccessoryDto({
    required this.title,
    this.pictureUrl,
    required this.price,
    this.url,
    required this.wishlist,
    required this.categoryId,
    this.subcategoryId,
    required this.slinghotQuantities,
  });

  Map<String, dynamic> toJson() {
    return {
      'title': title,
      'pictureUrl': pictureUrl,
      'price': price,
      'url': url,
      'wishlist': wishlist,
      'categoryId': categoryId,
      'subcategoryId': subcategoryId,
      'slinghotQuantities': slinghotQuantities,
    };
  }
}


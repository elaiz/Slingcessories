import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/accessory_dto.dart';
import '../models/create_accessory_dto.dart';
import '../models/category_dto.dart';
import '../models/create_category_dto.dart';
import '../models/subcategory_dto.dart';
import '../models/create_subcategory_dto.dart';
import '../models/slinghot_dto.dart';
import '../models/create_slingshot_dto.dart';
import '../models/user_dto.dart';
import '../models/create_user_dto.dart';

class ApiService {
  // For Android emulator, use: https://10.0.2.2:7289
  // For iOS simulator, use: https://localhost:7289
  // For web/Windows, use: https://localhost:7289
  // For physical device, use your computer's IP address
  static const String baseUrl = 'https://localhost:7289/api';

  // Accessories
  Future<List<AccessoryDto>> getAccessories({bool? wishlist}) async {
    var uri = Uri.parse('$baseUrl/Accessories');
    if (wishlist != null) {
      uri = uri.replace(queryParameters: {'wishlist': wishlist.toString()});
    }

    final response = await http.get(uri);

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => AccessoryDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load accessories');
    }
  }

  Future<AccessoryDto?> getAccessoryById(int id) async {
    final response = await http.get(Uri.parse('$baseUrl/Accessories/$id'));

    if (response.statusCode == 200) {
      return AccessoryDto.fromJson(json.decode(response.body));
    } else if (response.statusCode == 404) {
      return null;
    } else {
      throw Exception('Failed to load accessory');
    }
  }

  Future<AccessoryDto> createAccessory(CreateAccessoryDto dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/Accessories'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(dto.toJson()),
    );

    if (response.statusCode == 201) {
      return AccessoryDto.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to create accessory');
    }
  }

  Future<void> updateAccessory(int id, AccessoryDto accessory) async {
    final response = await http.put(
      Uri.parse('$baseUrl/Accessories/$id'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(accessory.toJson()),
    );

    if (response.statusCode != 204) {
      throw Exception('Failed to update accessory');
    }
  }

  Future<void> deleteAccessory(int id) async {
    final response = await http.delete(Uri.parse('$baseUrl/Accessories/$id'));

    if (response.statusCode != 204) {
      throw Exception('Failed to delete accessory');
    }
  }

  // Categories
  Future<List<CategoryDto>> getCategories() async {
    final response = await http.get(Uri.parse('$baseUrl/Categories'));

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => CategoryDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load categories');
    }
  }

  Future<CategoryDto?> getCategoryById(int id) async {
    final response = await http.get(Uri.parse('$baseUrl/Categories/$id'));

    if (response.statusCode == 200) {
      return CategoryDto.fromJson(json.decode(response.body));
    } else if (response.statusCode == 404) {
      return null;
    } else {
      throw Exception('Failed to load category');
    }
  }

  Future<CategoryDto> createCategory(CreateCategoryDto dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/Categories'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(dto.toJson()),
    );

    if (response.statusCode == 201) {
      return CategoryDto.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to create category');
    }
  }

  Future<void> updateCategory(int id, CategoryDto category) async {
    final response = await http.put(
      Uri.parse('$baseUrl/Categories/$id'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(category.toJson()),
    );

    if (response.statusCode != 204) {
      throw Exception('Failed to update category');
    }
  }

  Future<void> deleteCategory(int id) async {
    final response = await http.delete(Uri.parse('$baseUrl/Categories/$id'));

    if (response.statusCode != 204) {
      throw Exception('Failed to delete category');
    }
  }

  Future<List<SubcategoryDto>> getSubcategoriesByCategory(int categoryId) async {
    final response = await http.get(
      Uri.parse('$baseUrl/Categories/$categoryId/subcategories'),
    );

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => SubcategoryDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load subcategories');
    }
  }

  // Subcategories
  Future<List<SubcategoryDto>> getSubcategories() async {
    final response = await http.get(Uri.parse('$baseUrl/Subcategories'));

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => SubcategoryDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load subcategories');
    }
  }

  Future<List<SubcategoryDto>> getSubcategoriesByCategoryId(int categoryId) async {
    final response = await http.get(
      Uri.parse('$baseUrl/Subcategories/by-category/$categoryId'),
    );

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => SubcategoryDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load subcategories');
    }
  }

  Future<SubcategoryDto> createSubcategory(CreateSubcategoryDto dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/Subcategories'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(dto.toJson()),
    );

    if (response.statusCode == 201) {
      return SubcategoryDto.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to create subcategory');
    }
  }

  Future<void> updateSubcategory(int id, SubcategoryDto subcategory) async {
    final response = await http.put(
      Uri.parse('$baseUrl/Subcategories/$id'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(subcategory.toJson()),
    );

    if (response.statusCode != 204) {
      throw Exception('Failed to update subcategory');
    }
  }

  Future<void> deleteSubcategory(int id) async {
    final response = await http.delete(Uri.parse('$baseUrl/Subcategories/$id'));

    if (response.statusCode != 204) {
      throw Exception('Failed to delete subcategory');
    }
  }

  // Slingshots
  Future<List<SlinghotDto>> getSlingshots({String? userId}) async {
    var uri = Uri.parse('$baseUrl/Slingshots');
    if (userId != null && userId.isNotEmpty) {
      uri = uri.replace(queryParameters: {'userId': userId});
    }

    final response = await http.get(uri);

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => SlinghotDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load slingshots');
    }
  }

  Future<SlinghotDto?> getSlingshotById(int id) async {
    final response = await http.get(Uri.parse('$baseUrl/Slingshots/$id'));

    if (response.statusCode == 200) {
      return SlinghotDto.fromJson(json.decode(response.body));
    } else if (response.statusCode == 404) {
      return null;
    } else {
      throw Exception('Failed to load slingshot');
    }
  }

  Future<SlinghotDto> createSlingshot(CreateSlingshotDto dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/Slingshots'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(dto.toJson()),
    );

    if (response.statusCode == 201) {
      return SlinghotDto.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to create slingshot');
    }
  }

  Future<void> updateSlingshot(int id, SlinghotDto slingshot) async {
    final response = await http.put(
      Uri.parse('$baseUrl/Slingshots/$id'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(slingshot.toJson()),
    );

    if (response.statusCode != 204) {
      throw Exception('Failed to update slingshot');
    }
  }

  Future<void> deleteSlingshot(int id) async {
    final response = await http.delete(Uri.parse('$baseUrl/Slingshots/$id'));

    if (response.statusCode != 204) {
      throw Exception('Failed to delete slingshot');
    }
  }

  // Users
  Future<List<UserDto>> getUsers() async {
    final response = await http.get(Uri.parse('$baseUrl/Users'));

    if (response.statusCode == 200) {
      final List<dynamic> jsonList = json.decode(response.body);
      return jsonList.map((json) => UserDto.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load users');
    }
  }

  Future<UserDto?> getUserById(String id) async {
    final response = await http.get(Uri.parse('$baseUrl/Users/$id'));

    if (response.statusCode == 200) {
      return UserDto.fromJson(json.decode(response.body));
    } else if (response.statusCode == 404) {
      return null;
    } else {
      throw Exception('Failed to load user');
    }
  }

  Future<UserDto> registerUser(CreateUserDto dto) async {
    final response = await http.post(
      Uri.parse('$baseUrl/Users/register'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(dto.toJson()),
    );

    if (response.statusCode == 201) {
      return UserDto.fromJson(json.decode(response.body));
    } else {
      throw Exception('Failed to register user');
    }
  }

  Future<void> updateUser(String id, UserDto user) async {
    final response = await http.put(
      Uri.parse('$baseUrl/Users/$id'),
      headers: {'Content-Type': 'application/json'},
      body: json.encode(user.toJson()),
    );

    if (response.statusCode != 204) {
      throw Exception('Failed to update user');
    }
  }

  Future<void> deleteUser(String id) async {
    final response = await http.delete(Uri.parse('$baseUrl/Users/$id'));

    if (response.statusCode != 204) {
      throw Exception('Failed to delete user');
    }
  }
}


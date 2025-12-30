import 'package:flutter/foundation.dart';
import '../models/category_dto.dart';
import '../services/api_service.dart';

class CategoriesProvider with ChangeNotifier {
  final ApiService _apiService = ApiService();
  List<CategoryDto> _categories = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<CategoryDto> get categories => _categories;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> loadCategories() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      _categories = await _apiService.getCategories();
    } catch (e) {
      _errorMessage = 'Failed to load categories: $e';
      debugPrint(_errorMessage);
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> deleteCategory(int id) async {
    try {
      await _apiService.deleteCategory(id);
      await loadCategories();
    } catch (e) {
      _errorMessage = 'Failed to delete category: $e';
      notifyListeners();
    }
  }
}


import 'package:flutter/foundation.dart';
import '../models/accessory_dto.dart';
import '../services/api_service.dart';

class AccessoriesProvider with ChangeNotifier {
  final ApiService _apiService = ApiService();
  List<AccessoryDto> _accessories = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<AccessoryDto> get accessories => _accessories;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> loadAccessories({bool? wishlist}) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      _accessories = await _apiService.getAccessories(wishlist: wishlist);
    } catch (e) {
      _errorMessage = 'Failed to load accessories: $e';
      debugPrint(_errorMessage);
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> deleteAccessory(int id) async {
    try {
      await _apiService.deleteAccessory(id);
      await loadAccessories();
    } catch (e) {
      _errorMessage = 'Failed to delete accessory: $e';
      notifyListeners();
    }
  }
}


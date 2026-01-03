import 'package:flutter/foundation.dart';
import '../models/slinghot_dto.dart';
import '../models/create_slingshot_dto.dart';
import '../services/api_service.dart';

class SlingshotsProvider with ChangeNotifier {
  final ApiService _apiService = ApiService();
  List<SlinghotDto> _slingshots = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<SlinghotDto> get slingshots => _slingshots;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> loadSlingshots({String? userId}) async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      _slingshots = await _apiService.getSlingshots(userId: userId);
    } catch (e) {
      _errorMessage = 'Failed to load slingshots: $e';
      debugPrint(_errorMessage);
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> createSlingshot({
    required int year,
    required String model,
    required String color,
    String? userId,
  }) async {
    try {
      final dto = CreateSlingshotDto(
        year: year,
        model: model,
        color: color,
        userId: userId ?? '', // Will need to get from user state
      );
      await _apiService.createSlingshot(dto);
      await loadSlingshots();
    } catch (e) {
      _errorMessage = 'Failed to create slingshot: $e';
      notifyListeners();
    }
  }

  Future<void> updateSlingshot({
    required int id,
    required int year,
    required String model,
    required String color,
  }) async {
    try {
      final existingSlingshot = _slingshots.firstWhere((s) => s.id == id);
      final updatedSlingshot = SlinghotDto(
        id: id,
        year: year,
        model: model,
        color: color,
        totalOwned: existingSlingshot.totalOwned,
        totalWishlist: existingSlingshot.totalWishlist,
        totalAccessories: existingSlingshot.totalAccessories,
      );
      await _apiService.updateSlingshot(id, updatedSlingshot);
      await loadSlingshots();
    } catch (e) {
      _errorMessage = 'Failed to update slingshot: $e';
      notifyListeners();
    }
  }

  Future<void> deleteSlingshot(int id) async {
    try {
      await _apiService.deleteSlingshot(id);
      await loadSlingshots();
    } catch (e) {
      _errorMessage = 'Failed to delete slingshot: $e';
      notifyListeners();
    }
  }
}

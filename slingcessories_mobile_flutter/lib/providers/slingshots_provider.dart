import 'package:flutter/foundation.dart';
import '../models/slinghot_dto.dart';
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


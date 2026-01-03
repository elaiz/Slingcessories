import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../theme/app_theme.dart';
import '../widgets/page_header.dart';
import '../providers/slingshots_provider.dart';
import '../models/slinghot_dto.dart';

class SlingshotsPage extends StatefulWidget {
  const SlingshotsPage({super.key});

  @override
  State<SlingshotsPage> createState() => _SlingshotsPageState();
}

class _SlingshotsPageState extends State<SlingshotsPage> {
  final Set<int> _expandedSlingshots = {};

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<SlingshotsProvider>().loadSlingshots();
    });
  }

  void _toggleExpand(int id) {
    setState(() {
      if (_expandedSlingshots.contains(id)) {
        _expandedSlingshots.remove(id);
      } else {
        _expandedSlingshots.add(id);
      }
    });
  }

  void _collapseAll() {
    setState(() {
      _expandedSlingshots.clear();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.backgroundLight,
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const PageHeader(title: 'Slingshots'),
            // Action buttons row
            Padding(
              padding: const EdgeInsets.all(16),
              child: Wrap(
                spacing: 8,
                runSpacing: 8,
                children: [
                  ElevatedButton.icon(
                    onPressed: () => _showAddDialog(context),
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('New Slingshot'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.accentGreen,
                      foregroundColor: Colors.white,
                    ),
                  ),
                  OutlinedButton.icon(
                    onPressed: _collapseAll,
                    icon: const Icon(Icons.unfold_less, size: 18),
                    label: const Text('Collapse All'),
                  ),
                  OutlinedButton.icon(
                    onPressed: () {
                      context.read<SlingshotsProvider>().loadSlingshots();
                    },
                    icon: const Icon(Icons.refresh, size: 18),
                    label: const Text('Refresh'),
                  ),
                ],
              ),
            ),
            Expanded(
              child: Consumer<SlingshotsProvider>(
                builder: (context, provider, child) {
                  if (provider.isLoading) {
                    return const Center(
                      child: CircularProgressIndicator(
                        color: AppColors.primaryDark,
                      ),
                    );
                  }

                  if (provider.errorMessage != null) {
                    return Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            Icons.error_outline,
                            size: 48,
                            color: AppColors.accentRed,
                          ),
                          const SizedBox(height: 16),
                          Text(
                            provider.errorMessage!,
                            style: const TextStyle(color: AppColors.accentRed),
                            textAlign: TextAlign.center,
                          ),
                          const SizedBox(height: 16),
                          OutlinedButton(
                            onPressed: () => provider.loadSlingshots(),
                            child: const Text('Retry'),
                          ),
                        ],
                      ),
                    );
                  }

                  if (provider.slingshots.isEmpty) {
                    return Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            Icons.category_outlined,
                            size: 64,
                            color: AppColors.textLight,
                          ),
                          const SizedBox(height: 16),
                          const Text(
                            'No slingshots found',
                            style: TextStyle(
                              fontSize: 18,
                              color: AppColors.textMedium,
                            ),
                          ),
                          const SizedBox(height: 8),
                          const Text(
                            'Add your first slingshot to get started',
                            style: TextStyle(color: AppColors.textLight),
                          ),
                        ],
                      ),
                    );
                  }

                  return ListView.builder(
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: provider.slingshots.length,
                    itemBuilder: (context, index) {
                      final slingshot = provider.slingshots[index];
                      final isExpanded = _expandedSlingshots.contains(slingshot.id);
                      return _buildSlingshotCard(context, provider, slingshot, isExpanded);
                    },
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSlingshotCard(
    BuildContext context,
    SlingshotsProvider provider,
    SlinghotDto slingshot,
    bool isExpanded,
  ) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Header row with name and buttons
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Name and color
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Text(
                                slingshot.displayName,
                                style: const TextStyle(
                                  fontSize: 18,
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.textDark,
                                ),
                              ),
                              const SizedBox(width: 8),
                              Text(
                                '(${slingshot.color})',
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: AppColors.textMedium,
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          // Price row
                          Wrap(
                            spacing: 16,
                            runSpacing: 8,
                            children: [
                              _buildPriceTag(
                                icon: Icons.check_circle_outline,
                                value: '\$${slingshot.totalOwned.toStringAsFixed(2)}',
                                color: AppColors.priceGreen,
                              ),
                              _buildPriceTag(
                                icon: Icons.star_outline,
                                value: '\$${slingshot.totalWishlist.toStringAsFixed(2)}',
                                color: AppColors.accentAmber,
                              ),
                              _buildPriceTag(
                                icon: Icons.folder_outlined,
                                value: '\$${slingshot.totalAccessories.toStringAsFixed(2)}',
                                color: AppColors.priceBlue,
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                    // Action buttons
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            // Show/Hide Accessories button
                            _buildActionButton(
                              icon: isExpanded ? Icons.visibility_off : Icons.visibility,
                              label: isExpanded ? 'Hide Accessories' : 'Show Accessories',
                              color: AppColors.primaryDark,
                              filled: isExpanded,
                              onPressed: () => _toggleExpand(slingshot.id),
                            ),
                            const SizedBox(width: 4),
                            // Edit button
                            _buildActionButton(
                              icon: Icons.edit,
                              label: 'Edit',
                              color: AppColors.textMedium,
                              onPressed: () => _showEditDialog(context, provider, slingshot),
                            ),
                            const SizedBox(width: 4),
                            // Delete button
                            _buildActionButton(
                              icon: Icons.delete,
                              label: 'Delete',
                              color: AppColors.accentRed,
                              onPressed: () => _confirmDelete(context, provider, slingshot.id),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ],
                ),
              ],
            ),
          ),
          // Expanded accessories section
          if (isExpanded) _buildAccessoriesSection(slingshot),
        ],
      ),
    );
  }

  Widget _buildPriceTag({
    required IconData icon,
    required String value,
    required Color color,
  }) {
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Icon(icon, size: 16, color: color),
        const SizedBox(width: 4),
        Text(
          value,
          style: TextStyle(
            color: color,
            fontWeight: FontWeight.w600,
          ),
        ),
      ],
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required String label,
    required Color color,
    required VoidCallback onPressed,
    bool filled = false,
  }) {
    if (filled) {
      return ElevatedButton.icon(
        onPressed: onPressed,
        icon: Icon(icon, size: 16),
        label: Text(label),
        style: ElevatedButton.styleFrom(
          backgroundColor: color,
          foregroundColor: Colors.white,
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          textStyle: const TextStyle(fontSize: 12),
        ),
      );
    }
    return OutlinedButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, size: 16),
      label: Text(label),
      style: OutlinedButton.styleFrom(
        foregroundColor: color,
        side: BorderSide(color: color),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        textStyle: const TextStyle(fontSize: 12),
      ),
    );
  }

  Widget _buildAccessoriesSection(SlinghotDto slingshot) {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.backgroundLight,
        border: Border(
          top: BorderSide(color: Colors.grey.shade300),
        ),
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header row
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Associated Accessories:',
                style: TextStyle(
                  fontSize: 14,
                  color: AppColors.textMedium,
                ),
              ),
              ElevatedButton.icon(
                onPressed: () {
                  // Add accessories functionality
                },
                icon: const Icon(Icons.add, size: 16),
                label: const Text('Add Accessories'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.accentGreen,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  textStyle: const TextStyle(fontSize: 12),
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          
          // Owned Accessories section
          Row(
            children: [
              Icon(Icons.check_circle_outline, size: 16, color: AppColors.priceGreen),
              const SizedBox(width: 8),
              const Text(
                'Owned Accessories',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w500,
                  color: AppColors.textMedium,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          if (slingshot.ownedAccessories != null && slingshot.ownedAccessories!.isNotEmpty)
            ...slingshot.ownedAccessories!.map((group) => _buildCategoryRow(group))
          else
            const Padding(
              padding: EdgeInsets.only(left: 24),
              child: Text(
                'No owned accessories',
                style: TextStyle(
                  fontSize: 13,
                  color: AppColors.textLight,
                  fontStyle: FontStyle.italic,
                ),
              ),
            ),
          
          const SizedBox(height: 16),
          
          // Wishlist Items section
          Row(
            children: [
              Icon(Icons.star, size: 16, color: AppColors.accentAmber),
              const SizedBox(width: 8),
              const Text(
                'Wishlist Items',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w500,
                  color: AppColors.textMedium,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          if (slingshot.wishlistAccessories != null && slingshot.wishlistAccessories!.isNotEmpty)
            ...slingshot.wishlistAccessories!.map((group) => _buildCategoryRow(group))
          else
            const Padding(
              padding: EdgeInsets.only(left: 24),
              child: Text(
                'No wishlist items',
                style: TextStyle(
                  fontSize: 13,
                  color: AppColors.textLight,
                  fontStyle: FontStyle.italic,
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildCategoryRow(SlingshotAccessoryGroup group) {
    return Padding(
      padding: const EdgeInsets.only(left: 24, bottom: 4),
      child: Row(
        children: [
          Icon(Icons.chevron_right, size: 16, color: AppColors.textLight),
          const SizedBox(width: 4),
          Text(
            group.categoryName,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w600,
              color: AppColors.textDark,
            ),
          ),
          const SizedBox(width: 8),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
            decoration: BoxDecoration(
              color: AppColors.textLight.withOpacity(0.2),
              borderRadius: BorderRadius.circular(10),
            ),
            child: Text(
              '${group.count}',
              style: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w600,
                color: AppColors.textMedium,
              ),
            ),
          ),
        ],
      ),
    );
  }

  void _showAddDialog(BuildContext context) {
    final yearController = TextEditingController();
    final modelController = TextEditingController();
    final colorController = TextEditingController();

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('New Slingshot'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: yearController,
                decoration: const InputDecoration(
                  labelText: 'Year',
                  hintText: 'e.g., 2023',
                ),
                keyboardType: TextInputType.number,
              ),
              const SizedBox(height: 16),
              TextField(
                controller: modelController,
                decoration: const InputDecoration(
                  labelText: 'Model',
                  hintText: 'e.g., SL',
                ),
              ),
              const SizedBox(height: 16),
              TextField(
                controller: colorController,
                decoration: const InputDecoration(
                  labelText: 'Color',
                  hintText: 'e.g., Navy Blue',
                ),
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              final year = int.tryParse(yearController.text);
              if (year != null && modelController.text.isNotEmpty && colorController.text.isNotEmpty) {
                context.read<SlingshotsProvider>().createSlingshot(
                  year: year,
                  model: modelController.text,
                  color: colorController.text,
                );
                Navigator.pop(context);
              }
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.accentGreen,
              foregroundColor: Colors.white,
            ),
            child: const Text('Create'),
          ),
        ],
      ),
    );
  }

  void _showEditDialog(BuildContext context, SlingshotsProvider provider, SlinghotDto slingshot) {
    final yearController = TextEditingController(text: slingshot.year.toString());
    final modelController = TextEditingController(text: slingshot.model);
    final colorController = TextEditingController(text: slingshot.color);

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Edit Slingshot'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: yearController,
                decoration: const InputDecoration(
                  labelText: 'Year',
                ),
                keyboardType: TextInputType.number,
              ),
              const SizedBox(height: 16),
              TextField(
                controller: modelController,
                decoration: const InputDecoration(
                  labelText: 'Model',
                ),
              ),
              const SizedBox(height: 16),
              TextField(
                controller: colorController,
                decoration: const InputDecoration(
                  labelText: 'Color',
                ),
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              final year = int.tryParse(yearController.text);
              if (year != null && modelController.text.isNotEmpty && colorController.text.isNotEmpty) {
                provider.updateSlingshot(
                  id: slingshot.id,
                  year: year,
                  model: modelController.text,
                  color: colorController.text,
                );
                Navigator.pop(context);
              }
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: AppColors.accentBlue,
              foregroundColor: Colors.white,
            ),
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _confirmDelete(BuildContext context, SlingshotsProvider provider, int id) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Slingshot'),
        content: const Text('Are you sure you want to delete this slingshot?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              provider.deleteSlingshot(id);
            },
            style: TextButton.styleFrom(foregroundColor: AppColors.accentRed),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }
}

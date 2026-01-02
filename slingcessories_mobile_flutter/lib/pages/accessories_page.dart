import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../theme/app_theme.dart';
import '../widgets/page_header.dart';
import '../providers/accessories_provider.dart';

class AccessoriesPage extends StatefulWidget {
  const AccessoriesPage({super.key});

  @override
  State<AccessoriesPage> createState() => _AccessoriesPageState();
}

class _AccessoriesPageState extends State<AccessoriesPage> {
  bool _showCards = true;
  bool _showWishlist = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<AccessoriesProvider>().loadAccessories();
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
            const PageHeader(title: 'Accessories'),
            // Action buttons row
            Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  ElevatedButton.icon(
                    onPressed: () => _showAddDialog(context),
                    icon: const Icon(Icons.add, size: 18),
                    label: const Text('Add Accessory'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.accentGreen,
                      foregroundColor: Colors.white,
                    ),
                  ),
                  const Spacer(),
                  // View toggle buttons
                  Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(6),
                      border: Border.all(color: AppColors.textLight),
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        _buildToggleButton(
                          icon: Icons.grid_view,
                          label: 'Cards',
                          isSelected: _showCards,
                          onTap: () => setState(() => _showCards = true),
                          isLeft: true,
                        ),
                        _buildToggleButton(
                          icon: Icons.view_list,
                          label: 'List',
                          isSelected: !_showCards,
                          onTap: () => setState(() => _showCards = false),
                          isLeft: false,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            // Filter tabs
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Row(
                children: [
                  _buildFilterChip(
                    label: 'All',
                    isSelected: !_showWishlist,
                    onTap: () {
                      setState(() => _showWishlist = false);
                      context.read<AccessoriesProvider>().loadAccessories();
                    },
                  ),
                  const SizedBox(width: 8),
                  _buildFilterChip(
                    label: 'Wishlist',
                    isSelected: _showWishlist,
                    onTap: () {
                      setState(() => _showWishlist = true);
                      context.read<AccessoriesProvider>().loadAccessories(wishlist: true);
                    },
                  ),
                ],
              ),
            ),
            const SizedBox(height: 8),
            Expanded(
              child: Consumer<AccessoriesProvider>(
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
                            onPressed: () => provider.loadAccessories(wishlist: _showWishlist),
                            child: const Text('Retry'),
                          ),
                        ],
                      ),
                    );
                  }

                  if (provider.accessories.isEmpty) {
                    return Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(
                            Icons.list_alt_outlined,
                            size: 64,
                            color: AppColors.textLight,
                          ),
                          const SizedBox(height: 16),
                          const Text(
                            'No accessories found',
                            style: TextStyle(
                              fontSize: 18,
                              color: AppColors.textMedium,
                            ),
                          ),
                          const SizedBox(height: 8),
                          const Text(
                            'Add your first accessory to get started',
                            style: TextStyle(color: AppColors.textLight),
                          ),
                        ],
                      ),
                    );
                  }

                  if (_showCards) {
                    return _buildCardGrid(provider);
                  } else {
                    return _buildListView(provider);
                  }
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildToggleButton({
    required IconData icon,
    required String label,
    required bool isSelected,
    required VoidCallback onTap,
    required bool isLeft,
  }) {
    return InkWell(
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: isSelected ? AppColors.primaryDark : Colors.transparent,
          borderRadius: BorderRadius.horizontal(
            left: isLeft ? const Radius.circular(5) : Radius.zero,
            right: !isLeft ? const Radius.circular(5) : Radius.zero,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              size: 16,
              color: isSelected ? Colors.white : AppColors.textMedium,
            ),
            const SizedBox(width: 4),
            Text(
              label,
              style: TextStyle(
                color: isSelected ? Colors.white : AppColors.textMedium,
                fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFilterChip({
    required String label,
    required bool isSelected,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(20),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        decoration: BoxDecoration(
          color: isSelected ? AppColors.primaryDark : Colors.transparent,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(
            color: isSelected ? AppColors.primaryDark : AppColors.textLight,
          ),
        ),
        child: Text(
          label,
          style: TextStyle(
            color: isSelected ? Colors.white : AppColors.textMedium,
            fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
          ),
        ),
      ),
    );
  }

  Widget _buildCardGrid(AccessoriesProvider provider) {
    return LayoutBuilder(
      builder: (context, constraints) {
        // Calculate number of columns based on screen width
        int crossAxisCount = 1;
        if (constraints.maxWidth > 900) {
          crossAxisCount = 3;
        } else if (constraints.maxWidth > 600) {
          crossAxisCount = 2;
        }

        return GridView.builder(
          padding: const EdgeInsets.all(16),
          gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: crossAxisCount,
            childAspectRatio: 0.75,
            crossAxisSpacing: 16,
            mainAxisSpacing: 16,
          ),
          itemCount: provider.accessories.length,
          itemBuilder: (context, index) {
            final accessory = provider.accessories[index];
            return Card(
              clipBehavior: Clip.antiAlias,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Image
                  AspectRatio(
                    aspectRatio: 16 / 9,
                    child: accessory.pictureUrl != null
                        ? Image.network(
                            accessory.pictureUrl!,
                            fit: BoxFit.cover,
                            errorBuilder: (context, error, stackTrace) {
                              return Container(
                                color: AppColors.backgroundLight,
                                child: const Center(
                                  child: Icon(
                                    Icons.image,
                                    size: 48,
                                    color: AppColors.textLight,
                                  ),
                                ),
                              );
                            },
                          )
                        : Container(
                            color: AppColors.backgroundLight,
                            child: const Center(
                              child: Icon(
                                Icons.image,
                                size: 48,
                                color: AppColors.textLight,
                              ),
                            ),
                          ),
                  ),
                  // Content
                  Expanded(
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            accessory.title,
                            style: const TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                              color: AppColors.textDark,
                            ),
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'Category: ${accessory.categoryName}',
                            style: const TextStyle(
                              fontSize: 12,
                              color: AppColors.textMedium,
                            ),
                          ),
                          if (accessory.subcategoryName != null)
                            Text(
                              'Subcategory: ${accessory.subcategoryName}',
                              style: const TextStyle(
                                fontSize: 12,
                                color: AppColors.textMedium,
                              ),
                            ),
                          const Spacer(),
                          Row(
                            children: [
                              const Text(
                                'Price: ',
                                style: TextStyle(
                                  fontSize: 14,
                                  color: AppColors.textMedium,
                                ),
                              ),
                              Text(
                                '\$${accessory.price.toStringAsFixed(2)}',
                                style: const TextStyle(
                                  fontSize: 14,
                                  fontWeight: FontWeight.bold,
                                  color: AppColors.textDark,
                                ),
                              ),
                              if (accessory.wishlist) ...[
                                const Spacer(),
                                const Icon(
                                  Icons.star,
                                  size: 16,
                                  color: AppColors.accentAmber,
                                ),
                              ],
                            ],
                          ),
                          const SizedBox(height: 8),
                          // Action buttons
                          Row(
                            mainAxisAlignment: MainAxisAlignment.end,
                            children: [
                              if (accessory.url != null)
                                _buildSmallButton(
                                  icon: Icons.open_in_new,
                                  label: 'Link',
                                  color: AppColors.textMedium,
                                  onTap: () {
                                    // Open link
                                  },
                                ),
                              _buildSmallButton(
                                icon: Icons.edit,
                                label: 'Edit',
                                color: AppColors.accentBlue,
                                onTap: () {
                                  // Edit
                                },
                              ),
                              _buildSmallButton(
                                icon: Icons.delete,
                                label: 'Delete',
                                color: AppColors.accentRed,
                                onTap: () => _confirmDelete(context, provider, accessory.id),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  Widget _buildSmallButton({
    required IconData icon,
    required String label,
    required Color color,
    required VoidCallback onTap,
  }) {
    return Padding(
      padding: const EdgeInsets.only(left: 4),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(4),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
          decoration: BoxDecoration(
            border: Border.all(color: color),
            borderRadius: BorderRadius.circular(4),
          ),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(icon, size: 12, color: color),
              const SizedBox(width: 4),
              Text(
                label,
                style: TextStyle(
                  fontSize: 11,
                  color: color,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildListView(AccessoriesProvider provider) {
    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: provider.accessories.length,
      itemBuilder: (context, index) {
        final accessory = provider.accessories[index];
        return Card(
          margin: const EdgeInsets.only(bottom: 12),
          child: ListTile(
            leading: accessory.pictureUrl != null
                ? ClipRRect(
                    borderRadius: BorderRadius.circular(4),
                    child: Image.network(
                      accessory.pictureUrl!,
                      width: 60,
                      height: 60,
                      fit: BoxFit.cover,
                      errorBuilder: (context, error, stackTrace) {
                        return Container(
                          width: 60,
                          height: 60,
                          color: AppColors.backgroundLight,
                          child: const Icon(Icons.image, color: AppColors.textLight),
                        );
                      },
                    ),
                  )
                : Container(
                    width: 60,
                    height: 60,
                    decoration: BoxDecoration(
                      color: AppColors.backgroundLight,
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: const Icon(Icons.image, color: AppColors.textLight),
                  ),
            title: Text(
              accessory.title,
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: AppColors.textDark,
              ),
            ),
            subtitle: Text(
              '${accessory.categoryName} • \$${accessory.price.toStringAsFixed(2)}',
              style: const TextStyle(color: AppColors.textMedium),
            ),
            trailing: Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (accessory.wishlist)
                  const Icon(Icons.star, color: AppColors.accentAmber, size: 20),
                IconButton(
                  icon: const Icon(Icons.delete, color: AppColors.accentRed),
                  onPressed: () => _confirmDelete(context, provider, accessory.id),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  void _showAddDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Add Accessory'),
        content: const Text('Add accessory feature coming soon!'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  void _confirmDelete(BuildContext context, AccessoriesProvider provider, int id) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Accessory'),
        content: const Text('Are you sure you want to delete this accessory?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              provider.deleteAccessory(id);
            },
            style: TextButton.styleFrom(foregroundColor: AppColors.accentRed),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }
}

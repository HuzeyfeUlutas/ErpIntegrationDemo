import { Modal } from 'antd';
import { ExclamationCircleOutlined } from '@ant-design/icons';

interface DeleteConfirmModalProps {
  open: boolean;
  title?: string;
  description?: string;
  loading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export function DeleteConfirmModal({
  open,
  title = 'Silme Onayı',
  description = 'Bu kaydı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.',
  loading = false,
  onConfirm,
  onCancel,
}: DeleteConfirmModalProps) {
  return (
    <Modal
      open={open}
      title={
        <span>
          <ExclamationCircleOutlined style={{ color: '#faad14', marginRight: 8 }} />
          {title}
        </span>
      }
      okText="Evet, Sil"
      cancelText="İptal"
      okButtonProps={{ danger: true, loading }}
      onOk={onConfirm}
      onCancel={onCancel}
      centered
    >
      <p style={{ marginLeft: 22 }}>{description}</p>
    </Modal>
  );
}

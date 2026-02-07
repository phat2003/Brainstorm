using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }//thuộc tính Category để truy cập các phương thức của CategoryRepository.
        ITopicRepository Topic { get; }//thuộc tính Topic để truy cập các phương thức của TopicRepository.
        void Save(); //phương thức Save để lưu các thay đổi vào cơ sở dữ liệu.
    }
}
